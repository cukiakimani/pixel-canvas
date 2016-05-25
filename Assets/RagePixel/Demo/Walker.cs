using UnityEngine;
using System.Collections;

public class Walker : MonoBehaviour {

    //Storing the reference to RagePixelSprite -component
    private IRagePixel ragePixel;
	 
    //enum for character state
    public enum WalkingState {Standing=0, WalkRight, WalkLeft};
    public WalkingState state = WalkingState.Standing;
    public RagePixelSprite arrowLeft;
    public RagePixelSprite arrowRight;

    //walking speed (pixels per second)
    public float walkingSpeed = 10f;

	void Start () {
        ragePixel = GetComponent<RagePixelSprite>();
	}

	void Update () {

        //Check the keyboard state and set the character state accordingly
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            state = WalkingState.WalkLeft;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            state = WalkingState.WalkRight;
        }
        else
        {
            state = WalkingState.Standing;
        }

        Vector3 moveDirection = new Vector3();
        
        switch (state)
        {
            case(WalkingState.Standing):
                //Reset the horizontal flip for clarity
                ragePixel.SetHorizontalFlip(false);
                ragePixel.PlayNamedAnimation("STAY", false);
                if (arrowLeft != null) arrowLeft.SetTintColor(Color.gray);
                if (arrowRight != null) arrowRight.SetTintColor(Color.gray);
                break;

            case (WalkingState.WalkLeft):
                //Flip horizontally. Our animation is drawn to walk right.
                ragePixel.SetHorizontalFlip(true);
                //PlayAnimation with forceRestart=false. If the WALK animation is already running, doesn't do anything. Otherwise restarts.
                ragePixel.PlayNamedAnimation("WALK", false);
                //Move direction. X grows right so left is -1.
                moveDirection = new Vector3(-1f, 0f, 0f);
                if (arrowLeft != null) arrowLeft.SetTintColor(Color.white);
                if (arrowRight != null) arrowRight.SetTintColor(Color.gray);
                break;

            case (WalkingState.WalkRight):
                //Not flipping horizontally. Our animation is drawn to walk right.
                ragePixel.SetHorizontalFlip(false);
                //PlayAnimation with forceRestart=false. If the WALK animation is already running, doesn't do anything. Otherwise restarts.
                ragePixel.PlayNamedAnimation("WALK", false);
                //Move direction. X grows right so left is +1.
                moveDirection = new Vector3(1f, 0f, 0f);
                if (arrowLeft != null) arrowLeft.SetTintColor(Color.gray);
                if (arrowRight != null) arrowRight.SetTintColor(Color.white);
                break;
        }

        //Move the sprite into moveDirection at walkingSpeed pixels/sec
        transform.Translate(moveDirection * Time.deltaTime * walkingSpeed);
	}
}
