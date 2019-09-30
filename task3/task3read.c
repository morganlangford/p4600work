#include <stdio.h>
#include <stdlib.h>
#include <math.h>

/*JAMES: Good but should check for sitations such as len = 0 
Ideally should also keep calculation and printing separate - what if you want
to reuse these functions in a program where printf doesn't apply - e.g. an embedded
device without a screen */

float findMean(float* input, int len){	// this function finds the mean
	if (len == 0){
		printf("\nLength of array of 0. Cannot do calculation.");
	}
	else {
		float mean = 0;

		for (int i=0; i<len; i++){
			mean += input[i];			// this is actually finding the sum
		}

		mean = mean/len;				// mean = sum/#values

		return mean;
	}
}

float findStdDev(float* input, float mean, int len){	// this function finds the std dev
	if (len == 0){
		printf("\nLength of array of 0. Cannot do calculation.");
	}
	else {
		float sum;

		for (int j=0; j<len; j++){
			sum += powf((input[j] - mean),2);		// sum of the squared differences
		}

		float stdDev = sqrt(sum/(len-1));			// equation for std dev using the mean from previous function

		return stdDev;
	}
}

int main(){
	FILE* dataFile;	// pointer towards our data file holding x and y for sine

	int counter = 0;
	float x; 
	float z;

	dataFile = fopen("sineWave.dat","r");	// open file holding sine values

	if (dataFile != NULL){ 					// Did the file successfully open? Otherwise DON'T WRITE 
		while (fscanf(dataFile,"%f %f",&x,&z) != EOF){ // counting how many rows are in the file
			counter++;
		}
		
		float y[counter]; 					// creating an array of length of file

		rewind(dataFile); 					// start at the beginning of the file again

		for (int i = 0; i<counter; i++){	// put all y values into an array called y
			fscanf(dataFile,"%f %f",&x,&y[i]);
		}
		
		float mean = findMean(y,counter); 	// send array y and length to function findMean
		float stdDev = findStdDev(y,mean,counter);			// call next function to find standard dev

		printf("\nMean = %f",mean);
		printf("\nStandard deviation = %f",stdDev);

	}
	else printf("\nCouldn't read file.");	// uf the file couldn't be opened...

	fclose(dataFile);						// close file
}