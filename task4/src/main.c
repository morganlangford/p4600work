#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <visa.h>

#include "smoothCurve.h"
#include "amplitude.h"

/* This is the main file for controlling the function generator and the oscilloscope
	and processing the data from the oscilloscope */

int main(){
	/* This is for testing purposes */

	/* Might not always know the length; you might have to run through
	the data and find it sometimes*/
	int len = 500;				
	float arr[len];
	float output[len-4];		// len - 4 because the moving average in the function loses 4 data points

	FILE* noisySineWave;		// this is the noisy wave I created that I want to read off of

	float x[len];				// need two separate arrays
	float y[len];				// one each for x, y values

	noisySineWave = fopen("noisySineWave.dat","r");		// read only file

	if (noisySineWave != NULL){							// this turns the input data into an array
		for (int i = 0; i < len; i++){	
			fscanf(noisySineWave,"%f %f",&x[i],&y[i]);
		}
	}
	else printf("\nnoisySineWave didn't open");

	smoothCurve(len,y,output); 					// calls function where it smooths the curve
	float amp = amplitude(len,y);					// this function finds the amplitude

	printf("\nAmplitude is %f",amp);

	/* Print out the new, smoothed data to a file */

	return 0;
}