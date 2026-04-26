<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\News; // Tells the controller about your database table

class NewsController extends Controller
{
    // ----------------------------------------------------
    // CREATE: Catches the form data and saves a new article
    // ----------------------------------------------------
    public function store(Request $request)
    {
        // 1. Make sure the user actually typed something valid
        $request->validate([
            'title' => 'required|max:255',
            'content' => 'required'
        ]);

        // 2. Create a new article in the database
        $news = new News;
        $news->title = $request->title;
        $news->content = $request->content;
        $news->published_at = now();
        $news->save();

        // 3. Send them back to the dashboard with a success message
        return redirect()->back()->with('success', 'News article posted successfully!');
    }

    // ----------------------------------------------------
    // DELETE: Finds a specific article and removes it
    // ----------------------------------------------------
    public function destroy($id)
    {
        // 1. Find the exact article by its database ID
        $news = News::findOrFail($id);
        
        // 2. Delete it permanently
        $news->delete();

        // 3. Send them back to the dashboard with a success message
        return redirect()->back()->with('success', 'News article deleted permanently!');
    }
}