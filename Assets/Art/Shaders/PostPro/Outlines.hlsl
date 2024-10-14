#ifndef SOBELOUTLINES_INCLUDED
#define SOBELOUTLINES_INCLUDED


void CalculateSobel_float(float TopLeft, float TopMid, float TopRight, float MidLeft, float MidRight, float BottomLeft, float BottomMid, float BottomRight, out float SobelValue)
{
    float SobelX = 0;
    float SobelY = 0;
    
    SobelX += TopLeft * -1;
    SobelX += MidLeft * -2;
    SobelX += BottomLeft * -1;
    SobelX += TopRight * 1;
    SobelX += MidRight * 2;
    SobelX += BottomRight * 1;
    
    SobelY += TopLeft * -1;
    SobelY += TopMid * -2;
    SobelY += TopRight * -1;
    SobelY += BottomLeft * 1;
    SobelY += BottomMid * 2;
    SobelY += BottomRight * 1;
    
    SobelValue = length(SobelX) + length(SobelY);

}


#endif 