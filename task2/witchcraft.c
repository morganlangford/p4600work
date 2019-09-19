#include <stdlib.h>	
#include <stdio.h>	
#include <math.h>

float findMean(float* input, int len){
	float mean = 0;

	for (int i=0; i<len; i++){
		mean += input[i];
	}

	mean = mean/len;

	return mean;
}

float findStdDev(float* input, float mean, int len){
	float sum;

	for (int j=0; j<len; j++){
		sum += powf((input[j] - mean),2);
	}

	float stdDev = sqrt(sum/(len-1));

	return stdDev;
}

int main(){
	float data [] ={0.068038, 0.173967, 0.439303, 0.615258, 0.789436, 0.770980, 0.890924, 1.034431, 0.955555, 0.991579, 0.919359, 0.857244, 0.680064, 0.558250, 0.473129, 0.278329, 0.027142, -0.151631, -0.454363, -0.534177, -0.803847, -0.882892, -0.996433, -0.919950, -1.068664, -1.000596, -0.997921, -0.909708, -0.607322, -0.611919, -0.380097, -0.127268};

	int sizeArray = sizeof(data)/sizeof(data[0]);

	float mean = findMean(data,sizeArray);
	float stdDev = findStdDev(data,mean,sizeArray);

	printf("\nMean = %f",mean);
	printf("\nStandard deviation = %f", stdDev);

	return 0;
}