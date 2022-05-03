using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchTest : MonoBehaviour
{
    public ParticleSystem TouchParticle;
    private float EmissionRate = 10f;
    private bool ShouldEmit = false;

    private void Start()
    {
        EmissionRate = TouchParticle.emissionRate;
    }
    
    void Update()
    {
        foreach (Touch touch in InputBehaviorTypes.touches)
        {
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
            {
                // Construct a ray from the current touch coordinates
                /*Ray ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray))
                {
                    // Create a particle if hit
                    Instantiate(TouchParticle, transform.position, transform.rotation);
                }*/
                ShouldEmit = true;
            }
        }

        if(ShouldEmit && TouchParticle.emissionRate != EmissionRate)
        {
            TouchParticle.emissionRate = EmissionRate;
        }
        else if(!ShouldEmit && TouchParticle.emissionRate != 0)
        {
            TouchParticle.emissionRate = 0f;
        }
    }
}
