
Representive photo of one region
-----
> Recently, my friends and I am doing a project to find the representative photo of one region. 

Method is:

		1. Divide all the photos to different clusters(Based on the distance < 75m)
		2. Reszie the photo into (1000*xx)
		3. Find the representative photo in each cluster.
This code is just for the - [x] 3rd medthod above.  
###Kitchen ready  

* Emgucv(Opencv For c#)  
	* SURF, FLANN
* Visual Studio
* YOUR BRAIN  

###Begin cook 
Input:
	You need to custom your own **folder name**(in my code, is "results") to this programe. which is a folder who contains all the folder which is one clauster with all pictures.
	P.S. For the example, "results" folder is in bin/Debug, because inside just **two empty cluster folder**, **MAYBE** you will get some **errors**, because I dont test this part. 
	`SO DONT FORGET TO PUT PHOTOS IN EACH CLUSTER FOLDER`
	
There are 5 functions in the file **ClusterMatch.cs**:
- Match(string pathOfFolder)
- ComputeSingleDescriptors(string fileName)
- ComputeMultipleDescriptors(string[] fileNames, out - IList<IndecesMapping> imap)
- ConcatDescriptors(IList<Matrix<float>> descriptors)
- **FindMatches**（...）    

###Cooking Book

Thanks to my friend, **Youness**, who do This for our presentation.
Four picturs to help understand.
![GitHub set up](https://cloud.githubusercontent.com/assets/7600637/11641048/59ffb87e-9d36-11e5-8f89-72a8ba3cab48.png)
![GitHub set up](https://cloud.githubusercontent.com/assets/7600637/11641047/580d4d24-9d36-11e5-8a9d-9a8b6f71c966.png)
![GitHub set up](https://cloud.githubusercontent.com/assets/7600637/11641044/553276b0-9d36-11e5-8f65-6bed30ca6039.png)
![GitHub set up](https://cloud.githubusercontent.com/assets/7600637/11641041/51f8957e-9d36-11e5-8e86-bf1cce3395e4.png)
