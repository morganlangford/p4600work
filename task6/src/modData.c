/* modify data taken */
#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <"functionGen.h">

/* This function is to find the max amplitude of the sine wave at a certain frequency*/

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

/* This program is going to take in any set of data points of varying length,
	reduce the noise and return a smooth set of data
	- done by averaging data */




void smoothCurve(int len, float* input, float* output){	
	float avg;			

	for(int i = 0; i < len-4; i++){ 		// this loop cycles through all the data							
		avg = 0;							// reset this every time

		for(int j = 0; j < 5; j++){			// this loop uses a moving average to average the data
			avg = (float)input[j+i]/5;		// is it supposed to be input[i+j]? Why?
		}

		output[i] = avg;					// fill new array
	}

	// Note to self: you cannot return an array. Use pointers instead
}


/* This function takes the amplitude at a certain freq and puts it in the file */

void cycleFreq(ViSession* handle,int lower_freq, int upper_freq, int num_points, char* file_name, float* smoothedArray){
	FILE* freq_vs_amp;
	freq_vs_amp = fopen(file_name,"w");	//fopen("file_name.DAT","w");
	float amp = 0;

	for(int i = lower_freq; i < upper_freq; i += (float)((upper_freq - lower_freq) / num_points)){
	// Basically it cycles from lower freq to upper freq using n steps where n = num_points			
		changeFreq(*handle, i, 5);
		amp = amplitude(num_points,smoothedArray);
		fprintf(freq_vs_amp,"\n%f %f",amp,i);
								
	}
	fclose(freq_vs_amp);
}