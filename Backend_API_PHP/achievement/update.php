<?php
include __DIR__ . '/../connection.php';
include __DIR__ . '/../constants.php';
include __DIR__ . '/../functions.php';
include __DIR__ . '/../headers.php';


try {
    // --- 1. JSON beolvasása és validálása ---
    $parsed = parseUpdateJson($allowedAchievement, 'user_id');
    // --- 2. SQL összeállítása ---
    $sql = "UPDATE user_achievements SET status =:status WHERE user_id = :user_id AND achievement_id = :achievement_id";
    // --- 3. Lekérdezés előkészítése és futtatása ---
    executeAndRespondForUpdate($pdo, $sql, $parsed["values"]);
} catch (Exception $e) {
    // --- 4. Hibakezelés ---
    sendErrorResponse($e);
}