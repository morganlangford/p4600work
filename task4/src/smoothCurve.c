#include <stdio.h>
#include <stdlib.h>
#include <math.h>

/* This program is going to take in any set of data points of varying length,
	reduce the noise and return a smooth set of data
	- done by averaging data */

void smooth(int len, float* input, float* smoothArray){	
	float avg;
	int len;
	int j;

	while(input[j] != NULL){					// this loop finds the length of the array
		j++;
	}

	len = j;

	/* Now we want to make a new array of length 4 less than the original array because 
	during the averaging we will lose four data points */
	float smoothArray[len-4];					

	for(int i = 0; i < len; i++){ 			// this loop cycles through all the data							
		avg = 0;							// reset every time			

		for(int j = 0; j < 5; j++){			// this loop averages the points
			avg += input[j]/5;
		}

		smoothArray[i] = avg;				// fill new array
	}

	// Note to self: you cannot return an array
}
