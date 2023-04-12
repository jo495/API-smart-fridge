using Fridge.Models;
namespace Fridge.Strategies;

interface WhatToCookStrategy
{
    public List<Recipe> WhatToCook();
    
}