/* this file is specifically meant for testing/trouble shooting */

#include <stdio.h>

/* How to get user input in C */
int main(void) 
{
    char name[20];
    printf("Hello. What's your name?\n");
    //scanf("%s", &name);  - deprecated
    fgets(name,20,stdin);
    printf("Hi there, %s", name);
    return 0;
}