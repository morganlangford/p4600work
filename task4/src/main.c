#include <stdio.h>
#include <stdlib.h>
#include <math.h>

#include "smoothCurve.h"
#include "amplitude.h"

/* This is the main file for controlling the function generator and the oscilloscope
	and processing the data from the oscilloscope */

int main(){
	/* This is for testing purposes */
	int len = 500;
	float arr[len];
	float output[len];

	FILE* noisySineWave;

	float x[len];
	float y[len];

	noisySineWave = fopen("noisySineWave.dat","r");

	if (noisySineWave != NULL){
		for (int i = 0; i < len; i++){	
			fscanf(noisySineWave,"%f %f",&x[i],&y[i]);
		}
	else print("\nnoisySineWave didn't open");

	smoothCurve(len,arr,output); 
	float amp = amplitude(output);

	printf("\nAmplitude is %f",amp);

	return 0;
}