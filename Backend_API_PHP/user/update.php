<?php
include __DIR__ . '/../connection.php';
include __DIR__ . '/../constants.php';
include __DIR__ . '/../functions.php';
include __DIR__ . '/../headers.php';

try {
    // --- 1. JSON beolvasása és validálása ---
    $parsed = parseUpdateJson($allowedUser, 'user_id');
    // --- 2. Speciális validációk ---
    validateUserFields($parsed['values'], $bannedPhrases);
    // --- 3. SQL összeállítása ---
    $sql = "UPDATE users SET {$parsed['set']} WHERE user_id = :user_id";
    // --- 4. Lekérdezés előkészítése és futtatása ---
    executeAndRespondForUpdate($pdo, $sql, $parsed["values"]);
} catch (Exception $e) {
    // --- 5. Hibakezelés ---
    sendErrorResponse($e);
}

