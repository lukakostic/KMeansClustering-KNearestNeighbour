# KMeansClustering and KNearestNeighbour
Minimal implementation of KMeansClustering and KNearestNeighbour classifiers (simple machine learning), in C#

Fairly simple and commented, check KMeansClustering() and KNearestNeighbour() methods.

Nearest Neighbour - place each element into n-dimensional space (dimension based on feature count), then when you need to classify a custom element, you also place it in n-dimensional space and find the closest one with euclidian distance (pythagoras). Can also be used for regression.

Clustering - same as Nearest Neighbour but you form clusters for each type and find closest cluster center like you did for nearest Neighbour

Has the sample trainging data i made up, as shown in picture(s).

Clustering has no much use here as i have only 1 element per type, but if you have a better dataset then it can shine

Both predict the type/class of an element based on its features

![before](https://user-images.githubusercontent.com/41348897/44055834-d071a1ec-9f46-11e8-9f68-c296cbc3d154.png)

![after](https://user-images.githubusercontent.com/41348897/44055833-d047f374-9f46-11e8-9419-7cc4a5371fa2.png)
