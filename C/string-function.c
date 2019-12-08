#include <stdio.h>

void Display(char ch[]);	//fu

int main()
{
	char c[50];
	printf("Enter string: ");
	gets(c);
	Display(c);	//prints output using 'puts'
	return 0;
}

void Display(char ch[])
{
	printf("String Output: ");
	puts(ch);
}