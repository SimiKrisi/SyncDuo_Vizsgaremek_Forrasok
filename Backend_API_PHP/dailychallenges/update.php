<?php
include __DIR__ . '/../connection.php';
include __DIR__ . '/../constants.php';
include __DIR__ . '/../functions.php';
include __DIR__ . '/../headers.php';

try {
    // --- 1. JSON beolvasása és validálása ---
    $parsed = parseUpdateJson($allowedDailys, 'id');
    // --- 2. SQL összeállítása ---
    $sql = "UPDATE daily_challenges SET {$parsed['set']} WHERE dailyc_id = :id";
    // --- 3. Frissítés végrehajtása és válasz ---
    executeAndRespondForUpdate($pdo, $sql, $parsed["values"]);

} catch (Exception $e) {
    // --- 4. Hibakezelés ---
    sendErrorResponse($e);
}