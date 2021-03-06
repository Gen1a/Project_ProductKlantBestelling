﻿using BusinessLayer.Exceptions;
using BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Managers
{
    public class BestellingManager
    {
        private Dictionary<long, Bestelling> _bestellingen = new Dictionary<long, Bestelling>();
        public IReadOnlyList<Bestelling> GeefBestellingen()
        {
            return new List<Bestelling>(_bestellingen.Values).AsReadOnly();
        }
        public void VoegBestellingToe(Bestelling bestelling)
        {
            if (_bestellingen.ContainsKey(bestelling.BestellingId))
            {
                throw new BestellingManagerException("VoegBestellingToe");
            }
            else
            {
                _bestellingen.Add(bestelling.BestellingId, bestelling);
            }
        }
        public void VerwijderBestelling(Bestelling bestelling)
        {
            if (!_bestellingen.ContainsKey(bestelling.BestellingId))
            {
                throw new BestellingManagerException("VerwijderBestelling");
            }
            else
            {
                _bestellingen.Remove(bestelling.BestellingId);
            }
        }
        public Bestelling GeefBestelling(int bestellingId)
        {
            if (!_bestellingen.ContainsKey(bestellingId))
            {
                throw new BestellingManagerException("GeefBestelling");
            }
            else
            {
                return _bestellingen[bestellingId];
            }
        }
    }
}
