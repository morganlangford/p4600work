// Shamelessly stolen from Michael H

/* This file creates a 'noisy' sine wave and prints out the data 
	into another word file. */

#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <time.h>

void main(){
	FILE* sineOutput;

	int sineWaveSize = 500;		// this is the number of elements
	float sineWave[sineWaveSize];
	float x[sineWaveSize];

	sineOutput = fopen("noisySineWave.dat","w");
	if (sineOutput != NULL){
		for (int i = 0; i < sineWaveSize; i++){
			x[i] = M_PI * i/64;
			float rand1 = (float)rand()/(float)RAND_MAX;
			float rand2 = (float)rand()/(float)RAND_MAX;
			sineWave[i] = sin(x[i])+((rand1-rand2)/10);		// add noise between -0.1 and 0.1
		}
		fclose(sineOutput);
	}
	else printf("\nCould not open file");

	//rewind(sineOutput); 

	FILE* sineOutputData;

	sineOutputData = fopen("sineOutputData.dat","w");
	if (sineOutputData != NULL){
		for (int j = 0; j < sineWaveSize; j += 2){
			fprintf(sineOutputData,"\n%f %f",x[j],x[j+1]);
		}
	}
	fclose(sineOutputData);

	/*	The next section prints the x and y values to two separate files 
		to make it easier to graph in excel (where I'm just checking to 
		make sure Michael's code works 
		PS thanks Michael for saving my lazy ass */
	/*
	FILE* x_output;
	FILE* y_output;

	y_output = fopen("y_output.DAT","w");
	x_output = fopen("x_output.DAT","w");

	if(y_output != NULL && x_output != NULL){
		for (int j = 0; j < sineWaveSize; j++){
			fprintf(x_output,"\n%f",x[j]);
			fprintf(y_output,"\n%f",sineWave[j]);
		}
	}
	fclose(x_output);
	fclose(y_output);

	// ok I guess that worked
	*/
}