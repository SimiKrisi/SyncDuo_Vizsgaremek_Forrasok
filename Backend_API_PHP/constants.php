<?php

// --- API kulcsok ---
$gameApiKey = "game_api_key_123"; // Ez a játék alkalmazás kulcsa
$adminApiKey = "admin_api_key_456"; // Ez az admin alkalmazás kulcsa

// --- Engedélyezett mezők és tiltott kifejezések definiálása ---
$bannedPhrases = ["korte", "alma"];
$allowedUser = ["user_id", "game_id", "google_id", "email", "display_name", "status"];
$allowedUserSelectGame = ["user_id", "game_id", "display_name", "status"];
$allowedUserSelectAdmin = ["user_id", "game_id", "google_id", "email", "display_name", "status"];

$allowedAchievement = ["user_id", "status", "achievement_id"];
$allowedAchievementSelectGame = ["user_id", "status", "achievement_id"];
$allowedAchievementSelectAdmin = ["user_id", "status", "achievement_id"];

$allowedDailyCR = ["user_id", "dailyc_time", "date"];
$allowedDailyCRSelectGame = ["user_id", "dailyc_time", "date"];
$allowedDailyCRSelectAdmin = ["user_id", "dailyc_time", "date"];

$allowedSpeedrun = ["user_id", "speedrun_amount", "date"];
$allowedSpeedrunSelectGame = ["user_id", "speedrun_amount", "date"];
$allowedSpeedrunSelectAdmin = ["user_id", "speedrun_amount", "date"];

$allowedShop = ["user_id", "item_number"];
$allowedShopSelectGame = ["user_id", "item_number"];
$allowedShopSelectAdmin = ["user_id", "item_number"];

$allowedDailys = ["dailyc_id", "data_json"];
$allowedDailysSelectGame = ["dailyc_id", "data_json"];
$allowedDailysSelectAdmin = ["dailyc_id", "data_json"];

$allowedStats = ["user_id", "coins", "best_speedrun_amount", "best_dailyc_time", "levels_completed"];
$allowedStatsSelectGame = ["user_id", "coins", "best_speedrun_amount", "best_dailyc_time", "levels_completed"];
$allowedStatsSelectAdmin = ["user_id", "coins", "best_speedrun_amount", "best_dailyc_time", "levels_completed"];

// Tiltott gyökerek
$roots = [
    // Angol
    "sex", "porn", "genital", "masturb", "prostitut",
    "excrement", "feces", "urine", "anus",

    // Magyar
    "fasz", "pina", "kurva","buzi", "geci","szar", "picsa", "segg", "basz", "ribanc", "szop", "hugy",

    // Német
    "fick", "fotze", "schwanz", "hure", "wichs", "pimmel", "arsch", "scheisse",

    // Olasz
    "cazzo", "vaffanculo", "stronzo", "merda", "troia", "puttana",

    // Spanyol
    "puta", "mierda", "coño", "pendejo", "joder", "gilipollas",

    // Francia
    "putain", "merde", "con", "connard", "salop", "enculé",
    
    ];

// Regex minták
$patterns = [
    // Angol
    "/f+u+[ck]+/i",
    "/sh+i+t+/i",
    "/b+i+t+c+h+/i",
    "/a+s+s+/i",
    "/d+i+c+k+/i",
    "/p+e+n+i+s+/i",
    "/v+a+g+i+n+a+/i",
    "/c+u+n+t+/i",
    "/s+e+x+/i",
    "/p+o+r+n+/i",

    // Magyar
    "/f+a+s+z+/i",
    "/p+i+n+a+/i",
    "/k+u+r+v+/i",
    "/b+u+z+/i",
    "/g+e+c+/i",
    "/s+z+a+r+/i",
    "/p+i+c+s+/i",
    "/s+e+g+/i",
    "/b+a+s+/i",
    "/r+i+b+a+n+c+/i",
    "/s+z+o+p+/i",
    "/h+u+g+y+/i",

    // Német
    "/f+i+c+k+/i",
    "/s+c+h+w+a+n+z+/i",
    "/h+u+r+e+/i",
    "/w+i+c+h+s+/i",
    "/p+i+m+m+e+l+/i",
    "/a+r+s+c+h+/i",
    "/s+c+h+e+i+s+s+e+/i",

    // Olasz
    "/c+a+z+z+o+/i",
    "/v+a+f+f+a+n+c+u+l+o+/i",
    "/s+t+r+o+n+z+o+/i",
    "/m+e+r+d+a+/i",
    "/t+r+o+i+a+/i",
    "/p+u+t+t+a+/i",

    // Spanyol
    "/p+u+t+a+/i",
    "/m+i+e+r+d+a+/i",
    "/c+o+ñ+o+/i",
    "/p+e+n+d+e+j+o+/i",
    "/j+o+d+e+r+/i",
    "/g+i+l+i+p+o+l+l+a+s+/i",

    // Francia
    "/p+u+t+a+i+n+/i",
    "/m+e+r+d+e+/i",
    "/c+o+n+/i",
    "/c+o+n+n+a+r+d+/i",
    "/s+a+l+o+p+/i",
    "/e+n+c+u+l+é+/i",
];

// --- PDO hibakezelés beállítása globálisan ---
$pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);