<?php
include __DIR__ . '/../connection.php';
include __DIR__ . '/../constants.php';
include __DIR__ . '/../functions.php';
include __DIR__ . '/../headers.php';

try {
    // --- 1. SQL összeállítása (teljes tábla lekérdezése) ---
    $sql = "SELECT * FROM leaderboard_dailyc ORDER BY dailyc_time ASC";
    // --- 2. Lekérdezés és válasz ---
    executeAndRespondForSelect($pdo, $sql, []);
} catch (Exception $e) {
    // --- 3. Hibakezelés ---
    sendErrorResponse($e);
}