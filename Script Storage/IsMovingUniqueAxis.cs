input_x = Input.GetAxisRaw("Horizontal");
            bool isMovingHorizontal = Mathf.Abs(input_x) > 0.5f;

            input_y = Input.GetAxisRaw("Vertical");
            bool isMovingVertical = Mathf.Abs(input_y) > 0.5f;

            bool isPlayerMoving = (input_x != 0 || input_y != 0);
            bool wasMovingVertical = false;

            if (isPlayerMoving)
            {   
                playerAnimator.SetFloat("input_x", input_x);
                playerAnimator.SetFloat("input_y", input_y);
                if(isMovingHorizontal && isMovingVertical)
                {   // If its moving in both directions, prioritize to move on the new one
                    if(wasMovingVertical)
                    {
                        // Move horizontally
                        Vector2 movement = new Vector2(input_x, 0f);
                        rb2d.MovePosition(rb2d.position + movement * speed * Time.fixedDeltaTime);
                    }
                    else
                    {
                        // Move Vertically
                        Vector2 movement = new Vector2(0f, input_y);
                        rb2d.MovePosition(rb2d.position + movement * speed * Time.fixedDeltaTime);
                    }
                }
                else if (isMovingHorizontal)
                {
                    // Move horizontal
                    Vector2 movement = new Vector2(input_x, 0f);
                    rb2d.MovePosition(rb2d.position + movement * speed * Time.fixedDeltaTime);

                    // Set last movement to Horizontal (!Vertical)
                    wasMovingVertical = false;
                }
                else if (isMovingVertical)
                {
                    // Move vertical
                    Vector2 movement = new Vector2(0f, input_y);
                    rb2d.MovePosition(rb2d.position + movement * speed * Time.fixedDeltaTime);
                    
                    // Set last movement to Vertical
                    wasMovingVertical = true;
                }
                else
                {
                    isPlayerMoving = false;
                }
            }