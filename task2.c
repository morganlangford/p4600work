/* Notes for Sept 12
Today we're gonna look at functions and pointers 

We looked at hexidecimal 

*/

#include <stdlib.h>	
#include <stdio.h>

int main(){
	int x;
	int y;
	int* p; // pointer to integer - instead of storing a number it stores an address

	x = 5;
	y = 7;
	p = &x; // make p point to the address of x

	printf("x=%d\ny=%d\np=%p",x,y,p);

	printf("\n*p=%d",*p); // when the * is in front, it means what it's pointed TO

	*p += 2; // this changes *p AND x!!
			// but it may crash computer -segmentation fault- so you have to be really careful about it
			// bUt you can change what it points to
	printf("\nx=%d\ny=%d\n*p=%d",x,y,*p);
}