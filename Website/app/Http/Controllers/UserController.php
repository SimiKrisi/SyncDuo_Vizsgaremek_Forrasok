<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\User;

class UserController extends Controller
{
    // This function flips the admin switch back and forth
    public function toggleAdmin($id)
    {
        $user = User::findOrFail($id);

        // SAFETY MEASURE: Stop the admin from accidentally demoting themselves!
        if (auth()->id() === $user->id) {
            return redirect()->back()->with('error', 'You cannot demote yourself!');
        }

        // Flip the boolean (If it's true, make it false. If it's false, make it true)
        $user->is_admin = !$user->is_admin;
        $user->save();

        // Figure out the right success message
        $statusMessage = $user->is_admin ? 'promoted to Admin' : 'demoted to Player';

        return redirect()->back()->with('success', "User successfully {$statusMessage}!");
    }
    public function destroy($id)
    {
        $user = User::findOrFail($id);

        // SAFETY MEASURE: Stop the admin from deleting themselves!
        if (auth()->id() === $user->id) {
            return redirect()->back()->with('error', 'You cannot delete your own account!');
        }

        // Delete the user from the database
        $user->delete();

        return redirect()->back()->with('success', 'User permanently deleted.');
    }
}