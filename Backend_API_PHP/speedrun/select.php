<?php
include __DIR__ . '/../connection.php';
include __DIR__ . '/../constants.php';
include __DIR__ . '/../functions.php';
include __DIR__ . '/../headers.php';

try {
    // --- API kulcs validálása ---
    $apiKeyType = validateApiKey();
    
    // --- 1. user_id kinyerése GET-ből ---
    $user_id = getIdFromRequest();
    if (!$user_id) {
        throw new Exception("Missing user_id");
    }
    // --- 2. Oszlopok validálása és összeállítása ---
    $allowedFields = getAllowedSelectFields('speedrun', $apiKeyType);
    $columns = parseAndValidateColumnsForSelect($allowedFields);
    // --- 3. SQL összeállítása ---
    $sql = "SELECT $columns FROM leaderboard_speedrun WHERE user_id = :user_id";
    // --- 4. Lekérdezés és válasz ---
    executeAndRespondForSelect($pdo, $sql, ['user_id' => $user_id]);
} catch (Exception $e) {
    // --- 5. Hibakezelés ---
    sendErrorResponse($e);
}