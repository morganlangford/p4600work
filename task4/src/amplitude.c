#include <stdio.h>
#include <stdlib.h>
#include <math.h>

/* This function is to find the max amplitude of the sine wave */

float amplitude(float* arr){
	// Basically I have to find the max and min values and then find the difference
	float max,min,amp;

	/* make a loop that cycles through the data, checks if this point is bigger/smaller 
	than the max/min and if it is then make that the new max/min */
	int i = 0;

	while (arr[i] != NULL){
		if (arr[i] > max){
			max = arr[i];
		}
		if (arr[i] < min){
			min = arr[i];
		}
		i++;
	}

	if (abs(max) > abs(min)){
		amp = max;
	}
	else amp = min;

	return amp;
}