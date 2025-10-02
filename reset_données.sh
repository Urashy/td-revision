#!/bin/bash

# Script pour réinitialiser et alimenter la base de données

echo "🔄 Réinitialisation et alimentation de la base de données..."
echo ""

# Appel de l'API
response=$(curl -s -w "\n%{http_code}" -X POST http://localhost:5049/api/Seed/ResetAndSeed)

# Séparer le corps de la réponse et le code HTTP
http_code=$(echo "$response" | tail -n1)
body=$(echo "$response" | sed '$d')

# Vérifier le code de réponse
if [ "$http_code" -eq 200 ]; then
    echo "✅ Succès !"
    echo "$body" | python3 -m json.tool 2>/dev/null || echo "$body"
    echo ""
    echo "📊 Récupération des statistiques..."
    curl -s http://localhost:5049/api/Seed/GetStats | python3 -m json.tool 2>/dev/null
else
    echo "❌ Erreur (Code HTTP: $http_code)"
    echo "$body"
fi

echo ""
echo "✨ Terminé !"