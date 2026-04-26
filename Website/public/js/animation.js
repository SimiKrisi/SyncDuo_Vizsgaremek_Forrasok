const frames = [
"logo/syncduo-logo-shine1.svg",
"logo/syncduo-logo-shine2.svg",
"logo/syncduo-logo-shine3.svg",
"logo/syncduo-logo-shine4.svg",
"logo/syncduo-logo-shine5.svg",
"logo/syncduo-logo-shine6.svg",
"logo/syncduo-logo-shine7.svg",
"logo/syncduo-logo-shine8.svg",
"logo/syncduo-logo-shine9.svg",
"logo/syncduo-logo-shine10.svg",
"logo/syncduo-logo-shine11.svg",
"logo/syncduo-logo-shine12.svg"
];

let frame = 0;
const logo = document.getElementById("logoAnimation");

setInterval(()=>{

frame++;

if(frame>=frames.length){
frame=0;
}

logo.src = frames[frame];

},80);

document.addEventListener("DOMContentLoaded", () => {
    // Grab the banner
    const banner = document.querySelector('.news-banner');

    if (banner) {
        // Create the Tripwire
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                // When you scroll down and the banner enters the screen...
                if (entry.isIntersecting) {
                    // Add the 'show' class to trigger the smooth CSS transition
                    entry.target.classList.add('show');
                    
                    // Stop observing so it stays visible
                    observer.unobserve(entry.target); 
                }
            });
        }, {
            // Very forgiving: triggers as soon as 15% of the banner is visible
            threshold: 0.15 
        });

        // Start watching
        observer.observe(banner);
    }
});