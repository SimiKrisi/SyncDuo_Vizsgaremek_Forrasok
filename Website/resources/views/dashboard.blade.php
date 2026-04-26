<x-app-layout>
    <x-slot name="header">
        <h2 class="font-semibold text-xl text-gray-800 leading-tight">
            {{ __('SyncDuo Admin Dashboard') }}
        </h2>
    </x-slot>

    <div class="py-12">
        <div class="max-w-7xl mx-auto sm:px-6 lg:px-8">
            <div class="bg-white overflow-hidden shadow-sm sm:rounded-lg p-6">
                
                <h3 class="text-lg font-bold mb-4">Post a New Update</h3>

                @if(session('success'))
                    <div class="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded mb-4">
                        {{ session('success') }}
                    </div>
                @endif

                <form action="{{ route('news.store') }}" method="POST">
                    @csrf 
                    
                    <div class="mb-4">
                        <label class="block text-gray-700 text-sm font-bold mb-2">Title</label>
                        <input type="text" name="title" class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline" required placeholder="e.g., Update v1.2 is Live!">
                    </div>

                    <div class="mb-4">
                        <label class="block text-gray-700 text-sm font-bold mb-2">Content</label>
                        <textarea name="content" rows="4" class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline" required placeholder="Tell the players what is new..."></textarea>
                    </div>

                    <button type="submit" class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
                        Post News
                    </button>
                </form>

                <hr class="my-8 border-gray-300">
                
                <h3 class="text-lg font-bold mb-4">Manage Existing News</h3>

                <div class="space-y-4">
                    @foreach($newsList as $article)
                        <div class="flex justify-between items-center bg-gray-50 p-4 border rounded">
                            <div>
                                <h4 class="font-bold">{{ $article->title }}</h4>
                                <p class="text-sm text-gray-500">{{ \Carbon\Carbon::parse($article->published_at)->format('M j, Y') }}</p>
                            </div>
                            
                            <form action="{{ route('news.destroy', $article->id) }}" method="POST" onsubmit="return confirm('Are you sure you want to delete this article?');">
                                @csrf
                                @method('DELETE')
                                <button type="submit" class="bg-red-500 hover:bg-red-700 text-white font-bold py-1 px-3 rounded text-sm">
                                    Delete
                                </button>
                            </form>
                        </div>
                    @endforeach
                </div>

                <hr class="my-8 border-gray-300">
                
                <h3 class="text-lg font-bold mb-4">Manage Users</h3>

                @if(session('error'))
                    <div class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
                        {{ session('error') }}
                    </div>
                @endif

                <div class="space-y-4">
                    @foreach($users as $user)
                        <div class="flex justify-between items-center bg-gray-50 p-4 border rounded">
                            <div>
                                <h4 class="font-bold">{{ $user->name }}</h4>
                                <p class="text-sm text-gray-500">{{ $user->email }} | Role: 
                                    <span class="{{ $user->is_admin ? 'text-green-600 font-bold' : 'text-gray-500' }}">
                                        {{ $user->is_admin ? 'Administrator' : 'Player' }}
                                    </span>
                                </p>
                            </div>
                            
                            @if(auth()->id() !== $user->id)
                                <div class="flex space-x-2">
                                    
                                    <form action="{{ route('users.toggle', $user->id) }}" method="POST">
                                        @csrf
                                        @method('PATCH')
                                        @if($user->is_admin)
                                            <button type="submit" class="bg-yellow-500 hover:bg-yellow-700 text-white font-bold py-1 px-3 rounded text-sm">
                                                Remove Admin
                                            </button>
                                        @else
                                            <button type="submit" class="bg-green-500 hover:bg-green-700 text-white font-bold py-1 px-3 rounded text-sm">
                                                Make Admin
                                            </button>
                                        @endif
                                    </form>

                                    <form action="{{ route('users.destroy', $user->id) }}" method="POST" onsubmit="return confirm('Are you sure you want to permanently delete this user?');">
                                        @csrf
                                        @method('DELETE')
                                        <button type="submit" class="bg-red-600 hover:bg-red-800 text-white font-bold py-1 px-3 rounded text-sm">
                                            Delete
                                        </button>
                                    </form>

                                </div>
                            @else
                                <span class="text-gray-400 italic text-sm">This is you</span>
                            @endif
                        </div>
                    @endforeach
                </div>

            </div>
        </div>
    </div>
</x-app-layout>