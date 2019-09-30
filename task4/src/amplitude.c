#include <stdio.h>
#include <stdlib.h>
#include <math.h>

/* This function is to find the max amplitude of the sine wave */

float amplitude(int len, float* arr){
	// Basically I have to find the max and min values and then find the difference
	float max,min,amp;

	/* make a loop that cycles through the data, checks if this point is bigger/smaller 
	than the max/min and if it is then make that the new max/min */
	int i = 0;

	while (i < len){
		if (arr[i] > max){		// if this point is bigger than the current max
			max = arr[i];		// set it to the new max
		}
		if (arr[i] < min){		// if this point is smaller than the current min
			min = arr[i];		// set it to the new min
		}
		i++;					// check the next point
	}

	/* Check which abs value is bigger, return that as amplitude */
	if (abs(max) > abs(min)){	
		amp = max;
	}
	else amp = min;

	return amp;
}