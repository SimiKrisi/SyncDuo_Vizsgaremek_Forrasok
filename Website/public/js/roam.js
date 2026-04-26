document.addEventListener("DOMContentLoaded", () => {
    const kid1 = document.getElementById("kid1");
    const kid2 = document.getElementById("kid2");
    const kid3 = document.getElementById("kid3");
    const kid4 = document.getElementById("kid4");

    const screenWidth = window.innerWidth;
    const kidWidth = 100;
    
    const occupiedSpots = [];
    const minGap = 20; 

    // Global variables to track the mouse and who we are holding
    let activeDragKid = null;
    let mouseX = 0;
    let mouseY = 0;
    let lastMouseX = 0;
    let lastMouseY = 0;

    function getSafeSpawnX() {
        let attempts = 0;
        let randomX = 0;
        let isSafe = false;

        while (!isSafe && attempts < 50) {
            randomX = Math.random() * (screenWidth - kidWidth);
            isSafe = true;

            for (let i = 0; i < occupiedSpots.length; i++) {
                if (Math.abs(randomX - occupiedSpots[i]) < (kidWidth + minGap)) {
                    isSafe = false;
                    break;
                }
            }
            attempts++;
        }
        occupiedSpots.push(randomX);
        return randomX;
    }

    function createKidBrain(element, defaultSpeed) {
        if (!element) return null; 

        const brain = {
            element: element,
            x: getSafeSpawnX(),                          
            y: 0,                                        
            vy: 0,                                       
            gravity: 0.6,                                
            baseSpeed: defaultSpeed,
            defaultSpeed: defaultSpeed, // Remember their original speed
            direction: Math.random() > 0.5 ? 1 : -1,     
            state: 'running', // States: 'running', 'idle', 'looking', 'jumping', 'dragged'
            timer: 0,                                    
            lookTimer: 0,
            dragOffsetX: 0, // Where on the cube you clicked
            dragOffsetY: 0
        };

        // --- DRAG AND DROP EVENTS ---
        // We use 'pointerdown' so it works for both Mouse and Mobile Touch!
        element.addEventListener('pointerdown', (e) => {
            activeDragKid = brain;
            brain.state = 'dragged';
            brain.vy = 0; // Stop falling
            
            // Calculate where we grabbed them so they don't snap to the mouse center
            const rect = element.getBoundingClientRect();
            brain.dragOffsetX = e.clientX - rect.left;
            // We calculate Y relative to the bottom of the screen for our physics engine
            brain.dragOffsetY = window.innerHeight - e.clientY - brain.y;
            
            element.setPointerCapture(e.pointerId);
        });

        return brain;
    }

    const kids = [
        createKidBrain(kid1, 2.5),
        createKidBrain(kid2, 3.2), 
        createKidBrain(kid3, 2.0), 
        createKidBrain(kid4, 2.8)  
    ].filter(kid => kid !== null); 

    // Track mouse movement anywhere on the screen
    document.addEventListener('pointermove', (e) => {
        lastMouseX = mouseX;
        lastMouseY = mouseY;
        mouseX = e.clientX;
        mouseY = e.clientY;

        if (activeDragKid) {
            // Update position based on mouse minus where we grabbed them
            activeDragKid.x = mouseX - activeDragKid.dragOffsetX;
            
            // Convert mouse Y to our bottom-up Y coordinate system
            let newY = window.innerHeight - mouseY - activeDragKid.dragOffsetY;
            activeDragKid.y = Math.max(0, newY); // Don't let them drag below ground
        }
    });

    // When we let go of the mouse
    document.addEventListener('pointerup', () => {
        if (activeDragKid) {
            activeDragKid.state = 'jumping'; // Put them into falling physics mode
            
            // Calculate Throw Velocity! (Difference between last frame and this frame)
            let throwSpeedX = mouseX - lastMouseX;
            let throwSpeedY = lastMouseY - mouseY; // Inverted because screen Y goes down

            activeDragKid.vy = Math.min(Math.max(throwSpeedY * 0.8, -10), 25); // Cap throw height
            
            if (Math.abs(throwSpeedX) > 1) {
                activeDragKid.direction = throwSpeedX > 0 ? 1 : -1;
                activeDragKid.baseSpeed = Math.min(Math.abs(throwSpeedX * 0.5), 15); // Cap throw speed
            }

            activeDragKid.timer = 60;
            activeDragKid = null;
        }
    });

    function animateKids() {
        const currentScreenWidth = window.innerWidth;

        kids.forEach(kid => {
            // If we are actively dragging this kid, skip the AI and physics logic!
            if (kid.state !== 'dragged') {
                
                // 1. THE BRAIN (Only think if on the ground)
                if (kid.y === 0) {
                    kid.timer--; 

                    if (kid.timer <= 0) {
                        // Reset speed to normal in case they were thrown
                        kid.baseSpeed = kid.defaultSpeed; 
                        
                        const roll = Math.random();
                        if (roll < 0.45) {
                            kid.state = 'running';
                            kid.timer = Math.floor(Math.random() * 120) + 60; 
                        } else if (roll < 0.70) {
                            kid.state = 'idle';
                            kid.timer = Math.floor(Math.random() * 60) + 30;  
                        } else if (roll < 0.85) {
                            kid.state = 'looking';
                            kid.timer = Math.floor(Math.random() * 90) + 60;  
                            kid.lookTimer = 0; 
                        } else {
                            kid.state = 'jumping';
                            kid.vy = Math.random() * 5 + 12; 
                        }
                    }
                }

                // 2. HORIZONTAL MOVEMENT
                if (kid.state === 'running' || kid.state === 'jumping') {
                    kid.x += (kid.baseSpeed * kid.direction);
                } 
                else if (kid.state === 'looking') {
                    kid.lookTimer++;
                    if (kid.lookTimer > 30) {
                        kid.direction *= -1; 
                        kid.lookTimer = 0;   
                    }
                }

                // 3. VERTICAL MOVEMENT (Gravity)
                if (kid.state === 'jumping' || kid.y > 0) {
                    kid.y += kid.vy;       
                    kid.vy -= kid.gravity; 

                    if (kid.y <= 0) {
                        kid.y = 0;             
                        kid.vy = 0;            
                        kid.state = 'running'; 
                        kid.timer = 60;
                        // Add a tiny bounce effect when landing hard
                        if (kid.baseSpeed > 5) kid.baseSpeed *= 0.5; 
                    }
                }

                // 4. WALL SAFETY (Bounce off walls if thrown, otherwise just turn around)
                const currentKidWidth = kid.element.clientWidth || 100;
                if (kid.x <= 0) {
                    kid.x = 0;
                    kid.direction = 1;
                    if (kid.y === 0) kid.state = 'running'; 
                    kid.timer = 60;
                } else if (kid.x + currentKidWidth >= currentScreenWidth) {
                    kid.x = currentScreenWidth - currentKidWidth;
                    kid.direction = -1;
                    if (kid.y === 0) kid.state = 'running'; 
                    kid.timer = 60;
                }
            }

            // 5. DRAW IT (Applies to dragged kids AND AI kids)
            kid.element.style.transform = `translate3d(${kid.x}px, ${-kid.y}px, 0) scaleX(${kid.direction})`; 
        });

        requestAnimationFrame(animateKids);
    }

    requestAnimationFrame(animateKids);
}); 