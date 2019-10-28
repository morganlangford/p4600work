#ifndef __OSCILLOSCOPE_H
#define __OSCILLOSCOPE_H

#include <stdio.h>
#include <visa.h>

ViStatus getCurve(ViSession handle, char* dataBuffer, int npoints);
void identify(ViSession scopeHandle, char* resultBuffer, ViUInt32 resultCount);
void ifOpened(ViSession scopeHandle, ViUInt32 resultCount);
void ret(ViSession scopeHandle, ViUInt32 resultCount);

#endif
