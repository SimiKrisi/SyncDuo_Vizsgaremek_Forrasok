<?php
include __DIR__ . '/../connection.php';
include __DIR__ . '/../constants.php';
include __DIR__ . '/../functions.php';
include __DIR__ . '/../headers.php';

try {
    // --- API kulcs validálása ---
    validateApiKey();
    
    // --- 1. JSON beolvasása és validálása ---
    $parsed = parseAndValidateJsonForInsert($allowedUser);
    // --- 2. Speciális validációk ---
    validateUserFields($parsed['values'], $bannedPhrases);
    // --- 3. SQL összeállítása ---
    $sql = "INSERT INTO users (" . implode(", ", $parsed["columns"]) . ") VALUES (" . implode(", ", $parsed["placeholders"]) . ")";
    // --- 4. Végrehajtás és válasz ---
    executeAndRespondForInsert($pdo, $sql, $parsed["values"]);
} catch (Exception $e) {
    // --- 5. Hibakezelés ---
    sendErrorResponse($e);
}
