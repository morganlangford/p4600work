#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <visa.h>

#include "smoothCurve.h"
#include "amplitude.h"

/* This is the main file for controlling the function generator and the oscilloscope
	and processing the data from the oscilloscope */

void main(){
	FILE* noisy_wave_file; 				//this is my input file
	noisy_wave_file = fopen("noisy_sin_wave.DAT","r");		// open in read mode
	float temp_x;
	float temp_y;
	int counter = 0; 					

	/* Do the following only if the file opens correctly */
	if(noisy_wave_file != NULL){ 		//determines length of file
		while(fscanf(noisy_wave_file,"%f %f",&temp_x,&temp_y)!= EOF){
			counter++;
		}

		float noisy_wave[counter]; 			//array holds initial noisy values
		float smoothed_wave[counter]; 		//array holds smoothed values

		rewind(noisy_wave_file);			//restart from the beginning of the file

		for(int i = 0; i < counter; i++){ // this is reading the lines and placing them into x and y values
			fscanf(noisy_wave_file,"%f %f", &temp_x, &noisy_wave[i]); 
		}
		smoothCurve(counter, noisy_wave, smoothed_wave); //run the smoothing function


		//output my smoothed wave to a file so I can plot it
		FILE* smoothed_output_file;
		smoothed_output_file = fopen("Smoothed_data.DAT","w");	// open in write mode

		for(int j = 0; j < counter; j++){
			fprintf(smoothed_output_file,"\n%f",smoothed_wave[j]);
		}

		// always close files at the end
		fclose(noisy_wave_file);
		fclose(smoothed_output_file);

		//now to use the amplitude function
		float amp = amplitude(counter, smoothed_wave);
		printf("\nAmplitude is: %f",amp);
	}
	else printf("\nCould not find file");
}