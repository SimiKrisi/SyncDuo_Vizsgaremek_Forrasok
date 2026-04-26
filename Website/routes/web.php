<?php

use App\Http\Controllers\ProfileController;
use App\Http\Controllers\NewsController;
use App\Http\Controllers\UserController;
use Illuminate\Support\Facades\Route;
use App\Models\News;

Route::get('/', function () {
    // Grab the news from the database
    $allNews = News::latest('published_at')->get();

    // Pass it to the welcome page
    return view('welcome', ['newsList' => $allNews]);
});

Route::get('/dashboard', function () {
    $allNews = App\Models\News::latest('published_at')->get();
    
    // NEW: Grab all the registered users!
    $allUsers = App\Models\User::all(); 
    
    return view('dashboard', [
        'newsList' => $allNews, 
        'users' => $allUsers // Pass them to the view
    ]);
    })->middleware(['auth', 'verified', 'admin'])->name('dashboard');

Route::middleware(['auth', 'admin'])->group(function () {
    // NEW: The route to toggle admin status
    Route::patch('/users/{id}/toggle-admin', [UserController::class, 'toggleAdmin'])->name('users.toggle');
    // NEW: The route to delete a user
    Route::delete('/users/{id}', [UserController::class, 'destroy'])->name('users.destroy');
    Route::get('/profile', [ProfileController::class, 'edit'])->name('profile.edit');
    Route::patch('/profile', [ProfileController::class, 'update'])->name('profile.update');
    Route::delete('/profile', [ProfileController::class, 'destroy'])->name('profile.destroy');
    
    // NEW: The route that catches your form submission!
    Route::post('/news', [NewsController::class, 'store'])->name('news.store');
    
    // NEW: The route that catches the delete request!
    Route::delete('/news/{id}', [NewsController::class, 'destroy'])->name('news.destroy');
});

require __DIR__.'/auth.php';
