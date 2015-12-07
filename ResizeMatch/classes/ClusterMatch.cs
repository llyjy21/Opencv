using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.Flann;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using System.IO;
using Emgu.CV.CvEnum;

namespace ResizeMatch.classes
{
    class ClusterMatch
    {
        /// <summary>
        /// Main method.
        /// </summary>
        public string Match(string pathOfFolder)
        {
            // pathOfFolder is the path to the folder of photos
            DirectoryInfo d = new DirectoryInfo(pathOfFolder);

            // getting jpg files
            FileInfo[] Files = d.GetFiles("*.jpg");
            List<string> dbImagesList = new List<string>();
            foreach (FileInfo file in Files)
            {
                dbImagesList.Add(pathOfFolder + file.Name);
            }
            
            string[] dbImages = dbImagesList.ToArray();
            
            IList<IndecesMapping> imap = null;

            // compute descriptors for each image
            var dbDescsList = ComputeMultipleDescriptors(dbImages, out imap);

            // concatenate all DB images descriptors into single Matrix
            // it looks like all the oberseved images is on image, and dbDesec is the descriptor for this "specific" image
            Matrix<float> dbDescs = ConcatDescriptors(dbDescsList);

            // find the maximum value of similar properties of all the photos
            int maxSimilarities = 0;
            // the name of the most representative photo of all the photos
            string representivePic = null;

            // find the representative photo for each photo, who is voted most who is the most representative photo
            foreach (string modelImage in dbImages)
            {
                // compute descriptors for the model image
                Matrix<float> modelDescriptors = ComputeSingleDescriptors(modelImage);
                FindMatches(modelImage, dbDescs, modelDescriptors, ref imap);

                foreach (var img in imap)
                {
                    if (img.TempSimilarity > maxSimilarities)
                    {
                        maxSimilarities = img.TempSimilarity;
                        representivePic = img.fileName;
                    }
                    
                    img.TempSimilarity = 0;
                }

                foreach (var img in imap)
                {
                    if (img.fileName == representivePic)
                    {
                        img.Representative++;
                    }
                    
                }
                Console.WriteLine("----Info: Processing of " + modelImage.Substring(pathOfFolder.Length) + " is completed, it choose" + representivePic.Substring(pathOfFolder.Length));
                maxSimilarities = 0;
                representivePic = null;
            }

            int maxRepresentative = 0;
            // find which photo has the maximum representative value
            foreach (var img in imap)
            {
                if (img.Representative > maxRepresentative)
                {
                    maxRepresentative = img.Representative;
                    representivePic = img.fileName;
                    maxSimilarities = img.Similarity;

                }else if (img.Representative == maxRepresentative)
                {
                    if (img.Similarity > maxSimilarities)
                    {
                        maxRepresentative = img.Representative;
                        representivePic = img.fileName;
                        maxSimilarities = img.Similarity;
                    }
                    
                }
                
            }
            
            int returnStringLength = representivePic.Length - pathOfFolder.Length - 4;
            representivePic = representivePic.Substring(pathOfFolder.Length, returnStringLength);
            Console.WriteLine("Result: Respresentative photo is: " + representivePic);
            
            return representivePic;
        }

        /// <summary>
        /// Computes image descriptors.
        /// </summary>
        /// <param name="fileName">Image filename.</param>
        /// <returns>The descriptors for the given image.</returns>
        public Matrix<float> ComputeSingleDescriptors(string fileName)
        {
            Matrix<float> descs;
            int newWidth = 1000;
            // extract features from the image
            Image<Gray, Byte> img = new Image<Gray, byte>(fileName);
            try{
                int originalHeight = img.Height;
                int originalWidth = img.Width;
                double ratio = (double)newWidth / (double)originalWidth;
                int newHeight = Convert.ToInt32(originalHeight * ratio);
                img = img.Resize(newWidth, newHeight, INTER.CV_INTER_CUBIC, true);
                VectorOfKeyPoint keyPoints = detector.DetectKeyPointsRaw(img, null);
                descs = detector.ComputeDescriptorsRaw(img, null, keyPoints);
            }
            finally
            {
                if (img != null)((IDisposable)img).Dispose();
            }
            
            return descs;
        }

        /// <summary>
        /// Convenience method for computing descriptors for multiple images.
        /// Returned imap is filled with structures specifying which descriptor ranges in the concatenated matrix belong to what image. 
        /// </summary>
        /// <param name="fileNames">Filenames of images to process.</param>
        /// <param name="imap">List of IndecesMapping to hold descriptor ranges for each image.</param>
        /// <returns>List of descriptors for the given images.</returns>
        public IList<Matrix<float>> ComputeMultipleDescriptors(string[] fileNames, out IList<IndecesMapping> imap)
        {
            imap = new List<IndecesMapping>();

            IList<Matrix<float>> descs = new List<Matrix<float>>();

            int r = 0;

            for (int i = 0; i < fileNames.Length; i++)
            {
                var desc = ComputeSingleDescriptors(fileNames[i]);
                descs.Add(desc);

                imap.Add(new IndecesMapping()
                {
                    fileName = fileNames[i],
                    IndexStart = r,
                    IndexEnd = r + desc.Rows - 1
                });

                r += desc.Rows;
            }

            return descs;
        }

        /// <summary>
        /// Computes 'similarity' value (IndecesMapping.Similarity) for each image in the collection against our model image.
        /// </summary>
        /// <param name="dbDescriptors">Query image descriptor.</param>
        /// <param name="modelDescriptors">Consolidated db images descriptors.</param>
        /// <param name="images">List of IndecesMapping to hold the 'similarity' value for each image in the collection.</param>
        public void FindMatches(string modelImage, Matrix<float> dbDescriptors, Matrix<float> modelDescriptors, ref IList<IndecesMapping> imap)
        {
            var indices = new Matrix<int>(modelDescriptors.Rows, 2); // matrix that will contain indices of the 2-nearest neighbors found
            var dists = new Matrix<float>(modelDescriptors.Rows, 2); // matrix that will contain distances to the 2-nearest neighbors found

            // create FLANN index with 4 kd-trees and perform KNN search over it look for 2 nearest neighbours

            var flannIndex = new Index(dbDescriptors, 4);
            flannIndex.KnnSearch(modelDescriptors, indices, dists, 2, 24);

            for (int i = 0; i < indices.Rows; i++)
            {
                // filter out all inadequate pairs based on distance between pairs
                if (dists.Data[i, 1] < 0.1)
                {
                    // find image from the db to which current descriptor range belongs and increment similarity value.
                    // because the indices.Data[i, 0] is always the image itself, so we chose the nearest neighbours
                    foreach (var img in imap)
                    {
                        if (img.IndexStart <= indices.Data[i, 1] && img.IndexEnd >= indices.Data[i, 1] && modelImage != img.fileName)
                        {
                            img.TempSimilarity++;
                            img.Similarity++;
                            break;
                        }
                        
                    }
                }
            }
        }

        /// <summary>
        /// Concatenates descriptors from different sources (images) into single matrix.
        /// </summary>
        /// <param name="descriptors">Descriptors to concatenate.</param>
        /// <returns>Concatenated matrix.</returns>
        public Matrix<float> ConcatDescriptors(IList<Matrix<float>> descriptors)
        {
            int cols = descriptors[0].Cols;
            int rows = descriptors.Sum(a => a.Rows);

            float[,] concatedDescs = new float[rows, cols];

            int offset = 0;

            foreach (var descriptor in descriptors)
            {
                // append new descriptors
                Buffer.BlockCopy(descriptor.ManagedArray, 0, concatedDescs, offset, sizeof(float) * descriptor.ManagedArray.Length);
                offset += sizeof(float) * descriptor.ManagedArray.Length;
            }

            return new Matrix<float>(concatedDescs);
        }

        public class IndecesMapping
        {
            public int IndexStart { get; set; }
            public int IndexEnd { get; set; }
            public int TempSimilarity { get; set; }
            public int Similarity { get; set; }
            public int Representative { get; set; }
            public string fileName { get; set; }
        }

        private const double surfHessianThresh = 300;
        private const bool surfExtendedFlag = true;
        private SURFDetector detector = new SURFDetector(surfHessianThresh, surfExtendedFlag);
    }
}
