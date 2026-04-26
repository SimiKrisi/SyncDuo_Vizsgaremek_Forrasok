document.addEventListener("DOMContentLoaded", function(){

    const frames = [
        "opening-animation/syncduo-logo1.svg",
        "opening-animation/syncduo-logo2.svg",
        "opening-animation/syncduo-logo3.svg",
        "opening-animation/syncduo-logo4.svg",
        "opening-animation/syncduo-logo5.svg",
        "opening-animation/syncduo-logo6.svg",
        "opening-animation/syncduo-logo7.svg"
    ];

    const introImage = document.getElementById("introFrame");
    const overlay = document.getElementById("introOverlay");
    
    // NEW: Grab the wrapper and the button
    const wrapper = document.getElementById("heroContentWrapper");
    const downloadBtn = document.getElementById("downloadBtn");

    let frame = 0;

    /* PRELOAD */
    const preloaded = [];
    frames.forEach(src=>{
        const img = new Image();
        img.src = src;
        preloaded.push(img);
    });

    setTimeout(()=>{

        const animation = setInterval(()=>{
            frame++;

            if(frame >= frames.length){
                clearInterval(animation);

                // 1. Swap to still image
                introImage.src = "logo/syncduo-logo-shine1.svg";

                // 2. Animate the whole wrapper up
                wrapper.classList.add("logo-settled");

                // 3. Make overlay transparent to clicks
                overlay.style.pointerEvents = "none";

                // 4. NEW: Wait 1.5 seconds for the float to finish, then reveal the button!
                setTimeout(() => {
                    downloadBtn.classList.add("reveal");
                }, 1500);

                return;
            }

            introImage.src = frames[frame];
        }, 90);
    }, 200);
});