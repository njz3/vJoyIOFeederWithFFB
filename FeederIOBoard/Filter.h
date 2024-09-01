/**
 * Digital Filter
 */

#ifndef Filter_h
#define Filter_h
#include "Arduino.h"

#define REAL float

class Filter {

public:
  Filter(REAL Bcoef[], int Bsize, REAL Acoef[], int Asize)
  {
    _Bcoef = Bcoef;
    _Bsize = Bsize;
    _previousMeasures = (REAL*)malloc(sizeof(REAL)*_Bsize);
    if (_previousMeasures==NULL)
      Serial.println("ERROR MALLOC");
    _Acoef = Acoef;
    _Asize = Asize;
    _previousValues = (REAL*)malloc(sizeof(REAL)*_Asize);
    if (_previousValues==NULL)
      Serial.println("ERROR MALLOC");
    reset();
  };
  
  ~Filter()
  {
    if (_previousValues!=NULL)
      free(_previousValues);
    if (_previousMeasures!=NULL)
      free(_previousMeasures);
  };
  
  REAL Process(REAL value)
  {
    REAL _sum = 0;
    int i;
    
    // Shift inputs
    for(i = 0; i < _Bsize-1; i++) {
      _previousMeasures[i] = _previousMeasures[i+1];
      _sum += _previousMeasures[i]*_Bcoef[i];
    }
    if (_Bsize>=1) {
      _previousMeasures[i] = value;
      _sum += _previousMeasures[i]*_Bcoef[i];
    }
    for(i = 0; i < _Asize-1; i++) {
      _sum -= _previousValues[i]*_Acoef[i];
      _previousValues[i] = _previousValues[i+1];
     }
    if (_Asize>=1) {
      _sum -= _previousValues[i]*_Acoef[i];
      _previousValues[i] = _lastValue;
     }
    
    _lastValue = _sum;
    return _lastValue;
  };
  
  REAL GetLast()
  {
    return _lastValue;
  };
  
  void reset()
  {
    int i;
    for(i = 0; i < _Bsize; i++){
      _previousMeasures[i] = 0;
    }
    for(i = 0; i < _Asize; i++){
      _previousValues[i] = 0;
    }
    _lastValue = 0.0;
  };

private:
  REAL *_Bcoef;
  int _Bsize;
  REAL *_previousValues;
  
  REAL *_Acoef;
  int _Asize;
  REAL *_previousMeasures;

  REAL _lastValue;
};

#endif
