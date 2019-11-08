#include <stdio.h>
#include <stdlib.h>
#include <math.h>

/* This function is to find the max amplitude of the sine wave */

float amplitude(int len, float* arr){
	// Basically I have to find the max and min values and then find the difference
	float max = -256;
	float min = 256;

	/* make a loop that cycles through the data, checks if this point is bigger/smaller 
	than the max/min and if it is then make that the new max/min */
	int i = 1;					// start at 1 because I've already set max/min = arr[0]

	while (i < len-4){
		if (arr[i] > max){		// if this point is bigger than the current max
			max = arr[i];		// set it to the new max
		}
		if (arr[i] < min){		// if this point is smaller than the current min
			min = arr[i];		// set it to the new min
		}
		i++;					// check the next point
	}

	/* Check which abs value is bigger, return that as amplitude */
	
	return (max-min)/2;			// amplitude
}