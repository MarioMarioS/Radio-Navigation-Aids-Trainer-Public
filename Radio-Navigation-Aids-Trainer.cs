/*
DEMO ONLY
NOT FULL CODE BASE
THIS IS TO AVOID PIRACY AS THIS IS A COMMERCIAL APP.
*/

    float CurrentRadialCalculator(Vector3 orgin, Vector3 outto)
    {
        currentRadialFloat = 360 - Quaternion.FromToRotation(Vector3.up, outto - orgin).eulerAngles.z;
        return currentRadialFloat;
    }

    void PQRandomNumScenarioRoll()
    {

        AirplanePositionRandomNumberInt = UnityEngine.Random.Range(1, 9);

        while (!PQrandomNumList.Contains(AirplanePositionRandomNumberInt))
        {

            AirplanePositionRandomNumberInt = UnityEngine.Random.Range(1, 9);
            //Debug.Log("Reroll Due to Double");
        }

        PQrandomNumList.Remove(AirplanePositionRandomNumberInt);

        //Debug.Log("List Count" + PQrandomNumList.Count);

        if (PQrandomNumList.Count == 0)
        {
            PQRandomNumListConstructor();
            //Debug.Log("List Empty, reconstructed");
        }

        //Debug.Log(AirplanePositionRandomNumberInt);
    }

    float PositionQuizCurrentRadialUserGuessCalculator(Vector3 orgin, Vector3 outto)
    {
        PositionQuizCurrentRadialUserGuessFloat = 360 - Quaternion.FromToRotation(Vector3.up, outto - orgin).eulerAngles.z;
        return PositionQuizCurrentRadialUserGuessFloat;
    }

    void AirplaneThrottle()
    {
        if (AirplaneThrottleBtnSelectedBool == true)
        {

            //Set up a vector 3 of first position
            OldAirplanePositionVector3 = airplane.transform.localPosition;

            //Move the Airplane
            airplane.transform.Translate(Vector3.up * Time.deltaTime *5f);

            //Wind fucntion
            //Note the wind is applied inbetween written vectors for the line forumla
            //this avoids duplicate vector3 old and new
            WindForceProcess();

            //Set up a different vector 3 of airplanes new position after its been moved
            NewAirplanePositionVector3 = airplane.transform.localPosition;

            // testing if these mother fucking old new positions match, if they do fuck it
            if (OldAirplanePositionVector3 != NewAirplanePositionVector3)
            {
                // rise y and run x calculated individually for slope
                float LineFormulaSlopeXpart = (NewAirplanePositionVector3.x - OldAirplanePositionVector3.x);
                float LineFormulaSlopeYpart = (NewAirplanePositionVector3.y - OldAirplanePositionVector3.y);

                //NEED A IF SLOPE ZERO WORK AROUND

                // m is slope, which is (y2-y1)/(x2-x1)
                float LineFormulaMslope = (LineFormulaSlopeYpart / LineFormulaSlopeXpart);

                // Solving for B (y intercept) b = y - mx
                float LineFormulaBFloat = NewAirplanePositionVector3.y - (LineFormulaMslope * NewAirplanePositionVector3.x);

                // Just delcaring some variables for use on fucntion below
                float LineFormulaCalculatedY;
                float LineFormulaCalculatedX;

                void LineFormulaCalcuateYFunction()
                {
                    //ATTEMPTED TEST!!! Work around if slope 0. which should result in y = b
                    if (LineFormulaMslope == 0 || LineFormulaMslope == float.PositiveInfinity || LineFormulaMslope == float.NegativeInfinity)
                    {
                        LineFormulaCalculatedY = LineFormulaBFloat;
                    }
                    else
                    {
                        // Line Formula solve for Y
                        LineFormulaCalculatedY = (LineFormulaMslope * (LineFormulaCalculatedX)) + LineFormulaBFloat;
                    }
                }

                void LineFormulaCalcuateXFunction()
                {
                    // Line Formula slove for X
                    LineFormulaCalculatedX = (LineFormulaCalculatedY - LineFormulaBFloat) / LineFormulaMslope;
                }

                // General Note !!PLAY AREA DIMENSIONS!!
                // x -305 to 305 ... y -110 to -653

                // If statement for if the Airplane touches the Right or Left Wall of play area
                if (airplane.transform.localPosition.x > 310 || airplane.transform.localPosition.x < -310)
                {

                    LineFormulaCalculatedX = -1 * airplane.transform.localPosition.x;
                    LineFormulaCalcuateYFunction();

                    // if statement for when outside of play area vertically up
                    if (LineFormulaCalculatedY > -110)
                    {
                        LineFormulaCalculatedY = -110;
                        LineFormulaCalcuateXFunction();
                    }

                    // if statement for when outside of play area vertically down
                    if (LineFormulaCalculatedY < -653)
                    {
                        LineFormulaCalculatedY = -653;
                        LineFormulaCalcuateXFunction();
                    }
                    // Ok so what is happening is duing winds the aircraft is moving in an unpredicable pace, reusltng in errors in
                    // the slope calculation because being divided by zero due to vector 3 old and new being the same ie 1 - 1 = 0
                    airplane.transform.localPosition = new Vector3(LineFormulaCalculatedX, LineFormulaCalculatedY, 0);
                }

                //If statement for if the airplane touches the top or bottom wall of play area
                if (airplane.transform.localPosition.y > -105 || airplane.transform.localPosition.y < -658)
                {
                    // B = 0 in Line formula... infinity work around. FML
                    if (LineFormulaBFloat == float.PositiveInfinity || LineFormulaBFloat == float.NegativeInfinity)
                    {
                        if (airplane.transform.localPosition.y >= -110)
                        {
                            airplane.transform.localPosition = new Vector3(airplane.transform.localPosition.x, -653, 0);
                        }
                        else if (airplane.transform.localPosition.y <= -653)
                        {
                            airplane.transform.localPosition = new Vector3(airplane.transform.localPosition.x, -110, 0);
                        }
                    }
                    else
                    {
                        //setting Y figure for line formula to the oppisite side of impact
                        LineFormulaCalculatedY = -653;
                        if (airplane.transform.localPosition.y <= -653)
                        {
                            LineFormulaCalculatedY = -110;
                        }
                        LineFormulaCalcuateXFunction();

                        // if ends up past right side boundry
                        if (LineFormulaCalculatedX > 305)
                        {
                            LineFormulaCalculatedX = 305;
                            LineFormulaCalcuateYFunction();
                        }

                        //if ends up past left side boundry
                        if (LineFormulaCalculatedX < -305)
                        {
                            LineFormulaCalculatedX = -305;
                            LineFormulaCalcuateYFunction();
                        }
                     
                        airplane.transform.localPosition = new Vector3(LineFormulaCalculatedX, LineFormulaCalculatedY, 0);
                    }
                }

            }
            else
            {
                Debug.Log("Error in AirplaneThrottle Method");
            }
        }
        AirplaneThrottleColorAnimator();
    }
