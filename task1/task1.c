#include <stdlib.h>	/* The standard libary: a collection of common C functions. Needed in almost any useful program */
#include <stdio.h>	/* The standard input/output library: functions for inputting and outputting data. Needed in almost any useful program*/ 
#include <math.h>	/* Math library: common math functions. Needed for almost any program a physicist would write! remember to use the -lm option with your compiler when including this file */ 


int main()
{
	// Data given 
	float data [] ={0.068038, 0.173967, 0.439303, 0.615258, 0.789436, 0.770980, 0.890924, 1.034431, 0.955555, 0.991579, 0.919359, 0.857244, 0.680064, 0.558250, 0.473129, 0.278329, 0.027142, -0.151631, -0.454363, -0.534177, -0.803847, -0.882892, -0.996433, -0.919950, -1.068664, -1.000596, -0.997921, -0.909708, -0.607322, -0.611919, -0.380097, -0.127268};

	float mean;
	float std_dev;
	float sum;
	int size;

	size = sizeof(data)/sizeof(data[0]); 		// =128/32
	
	//First, calculate the mean
	for (int i=0; i<size; i++)
		mean += data[i];
	mean = mean/size;

	//Now, find standard dev
	for (int j=0; j<size; j++)
		sum += powf((data[j] - mean),2);
	std_dev = sqrt(sum/(size-1));

	printf("Mean = %f\n", mean);
	printf("Standard deviation = %f\n", std_dev);
	
	return 0;  // Functions which return a value MUST contain at least one return statement
}

