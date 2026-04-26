<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>SyncDuo Main</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Press+Start+2P&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="css/style.css">
</head>
<body>

    <nav class="navbar navbar-expand-lg navbar-dark fixed-top">
        <div class="container">
            <a class="navbar-brand" href="#">
                <img id="logoAnimation" src="logo/syncduo-logo-shine1.svg" class="navbar-logo" alt="SyncDuo Logo">
            </a>
            <button class="navbar-toggler" data-bs-toggle="collapse" data-bs-target="#menu">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse justify-content-end" id="menu">
                <ul class="navbar-nav align-items-center">
                    <li class="nav-item">
                        <a class="nav-link active" href="#heroImage">Home</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="#news">News</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link text-warning font-bold" href="#play">Play Demo</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link" href="#aboutus">About us</a>
                    </li>

                    @auth
                        @if(auth()->user()->is_admin)
                            <li class="nav-item">
                                <a class="nav-link text-warning font-bold" href="{{ url('/dashboard') }}">Dashboard</a>
                            </li>
                        @endif
                        
                        <li class="nav-item">
                            <form method="POST" action="{{ route('logout') }}" class="m-0">
                                @csrf
                                <a class="nav-link" href="#" onclick="event.preventDefault(); this.closest('form').submit();">Log Out</a>
                            </form>
                        </li>
                    @else
                        <li class="nav-item">
                            <a class="nav-link" href="{{ route('login') }}">Log in</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="{{ route('register') }}">Register</a>
                        </li>
                    @endauth
                </ul>
            </div>
        </div>
    </nav>

<section id="heroImage" class="hero-image-section position-relative">
        
        <div id="introOverlay" class="intro-overlay">
            <div id="heroContentWrapper" class="d-flex flex-column align-items-center hero-content-wrapper">
                <img id="introFrame" src="opening-animation/syncduo-logo1.svg" class="intro-logo" alt="Intro Animation">
                
                <button id="downloadBtn" class="download-btn mt-4">Download Game</button>
            </div>
        </div>

        <!-- <div class="container-fluid h-100">
            <div class="row h-100">
              <div class="col-lg-6 d-flex align-items-end justify-content-start pb-0">
                  <img src="images/2%20characters%20for%20main%20page.svg" alt="SyncDuo Characters" style="height: 80vh; width: auto; max-width: none; margin-bottom: -5px; margin-left: -100px; position: relative; z-index: 2;">
              </div>
            </div>
        </div> -->
        
        <div id="kids-container" class="w-100 position-absolute" style="bottom: 0; left: 0; height: 100%; pointer-events: none; z-index: 3;">  
            <img src="images/blue.svg" id="kid1" class="kid-cube" alt="Blue Kid">
            <img src="images/pink.svg" id="kid2" class="kid-cube" alt="Pink Kid">
            <img src="images/green.svg" id="kid3" class="kid-cube" alt="Green Kid">
            <img src="images/yellow.svg" id="kid4" class="kid-cube" alt="Yellow Kid">
        </div>
    </section>

<section id="news" class="vh-100 d-flex flex-column">
    
    <div class="news-banner mx-auto">
        <h2>A game that will keep you sharp</h2>
    </div>

    <div class="container flex-grow-1 d-flex flex-column align-items-center justify-content-center pb-5">
        
        <div id="newsCarousel" class="carousel slide w-100" data-bs-theme="dark">
            <div class="carousel-inner">
                
                @foreach($newsList as $index => $article)
                    <div class="carousel-item {{ $index == 0 ? 'active' : '' }}">
                        
                        <div class="news-card mx-auto mb-4">
                            <h3 class="news-title">{{ $article->title }}</h3>
                            <p class="news-date">{{ \Carbon\Carbon::parse($article->published_at)->format('F j, Y') }}</p>
                            <p class="news-content">{{ $article->content }}</p>
                        </div>

                    </div>
                @endforeach

            </div>
            
            <button class="carousel-control-prev" type="button" data-bs-target="#newsCarousel" data-bs-slide="prev">
                <span class="carousel-control-prev-icon" aria-hidden="true"></span>
                <span class="visually-hidden">Previous</span>
            </button>
            <button class="carousel-control-next" type="button" data-bs-target="#newsCarousel" data-bs-slide="next">
                <span class="carousel-control-next-icon" aria-hidden="true"></span>
                <span class="visually-hidden">Next</span>
            </button>
        </div>

    </div>
</section>

<section id="play" class="d-flex flex-column align-items-center justify-content-start" style="min-height: 100vh; padding-top: 120px; padding-bottom: 60px;">

    <div class="mb-4 text-center">
        <h2 style="color: #000; font-weight: bold; font-family: 'Press Start 2P', monospace; font-size: 1.8rem;">Play the DEMO!</h2>
    </div>

    <div style="border: 8px solid #000; border-radius: 4px; background: #000; box-shadow: 15px 15px 0px rgba(0, 0, 0, 0.4); width: 90%; max-width: 380px; aspect-ratio: 9 / 16; overflow: hidden; position: relative;">
        <iframe src="{{ asset('game/index.html') }}" style="position: absolute; top: 0; left: 0; width: 100%; height: 100%; border: none;" scrolling="no" allowfullscreen></iframe>
    </div>

    <p class="text-dark mt-4 text-center px-3" style="font-size: 0.85rem; font-weight: bold;">
        * Works best on desktop browsers with hardware acceleration enabled.
    </p>

</section>

    <section id="aboutus" class="features py-5">
        <div class="container">
            <h2 class="text-center mb-5">Game Features</h2>
            <div class="row text-center">
                <div class="col-md-4">
                    <h4>Co-op Gameplay</h4>
                    <p>Play together with friends.</p>
                </div>
                <div class="col-md-4">
                    <h4>Weapons</h4>
                    <p>Hundreds of weapons.</p>
                </div>
                <div class="col-md-4">
                    <h4>Boss Fights</h4>
                    <p>Epic boss battles.</p>
                </div>
            </div>
        </div>
    </section>
    
    <script src="js/animation.js" defer></script>
    <script src="js/introAnimation.js"></script>
    <script src="js/roam.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>

</body>
</html>