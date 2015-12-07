
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

![GitHub set up](http://zh.mweb.im/asset/img/set-up-git.gif)



