#include <stdio.h>
#include <stdlib.h>
#include <math.h>

/* This program is going to take in any set of data points of varying length,
	reduce the noise and return a smooth set of data
	- done by averaging data */

void smoothCurve(int len, float* input, float* output){	
	float avg, sum;	

	for(int i = 0; i < len-4; i++){ 		// this loop cycles through all the data							
		avg = 0;							// reset this every time

		for(int j = 0; j < 5; j++){			// this loop uses a moving average to average the data
			sum = (float)input[i+j]/5;		// scale element
			avg += sum;	
		}

		output[i] = avg;					// fill new array
	}

	// Note to self: you cannot return an array. Use pointers instead
}

