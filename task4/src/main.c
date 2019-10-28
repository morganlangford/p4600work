#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <visa.h>

#include "smoothCurve.h"
#include "amplitude.h"

/* This is the main file for controlling the function generator and the oscilloscope
	and processing the data from the oscilloscope */

void main(){
	/* 	Basically this main was just for testing to see if the code works 
		and does what it's supposed to do     */
	FILE* noisy_wave_file; //this is my input file
	noisy_wave_file = fopen("noisy_sin_wave.DAT","r");
	//smooth the noisy wave
	float temp_x;
	float temp_y;
	int counter=0; //to find number of elements in the noisy wave

	if(noisy_wave_file != NULL){ //this is reading all the lines in the file to determine the size
		while(fscanf(noisy_wave_file,"%f %f",&temp_x,&temp_y)!= EOF){
			counter++;
		}

	float noisy_wave[counter]; //make an array that holds all the y values
	rewind(noisy_wave_file);

		for(int i = 0; i < counter; i++){ // this is reading the lines and placing them into x and y values
			fscanf(noisy_wave_file,"%f %f", &temp_x, &noisy_wave[i]); //do I need &?
		}


		
	float smoothed_wave[counter]; //this holds the y values of the wave after filtering
	smoothCurve(noisy_wave, smoothed_wave, counter); //run the smoothing function


	//output my smoothed wave to a file so I can plot it
	FILE* smoothed_output_file;
	smoothed_output_file = fopen("Smoothed_data.DAT","w");
	for(int j = 0; j < counter; j++){
		fprintf(smoothed_output_file,"\n%f",smoothed_wave[j]);
	}

	fclose(noisy_wave_file);
	fclose(smoothed_output_file);
	//now to use the amplitude function
	float amp = 0;
	amp = amplitude(smoothed_wave, counter);
	printf("\nAmplitude is: %f",amp);
	}
	else printf("\nCould not find file");
}