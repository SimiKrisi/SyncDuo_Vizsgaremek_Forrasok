<?php

namespace App\Http\Middleware;

use Closure;
use Illuminate\Http\Request;
use Symfony\Component\HttpFoundation\Response;

class IsAdmin
{
    /**
     * Handle an incoming request.
     *
     * @param  Closure(Request): (Response)  $next
     */
    public function handle(Request $request, Closure $next): Response
    {
        // If they are logged in AND their 'is_admin' flag is true... let them in!
        if (auth()->check() && auth()->user()->is_admin) {
            return $next($request);
        }

        // Otherwise, kick them back to the homepage
        return redirect('/');
    }
}
