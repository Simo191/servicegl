import { Pipe, PipeTransform } from '@angular/core';

export interface StatusBadgeResult {
  label: string;
  color: string;
}

const STATUS_MAP: Record<string, StatusBadgeResult> = {
  // Restaurant orders
  'Received':      { label: 'Reçue',        color: 'info' },
  'Confirmed':     { label: 'Confirmée',    color: 'primary' },
  'Preparing':     { label: 'En préparation', color: 'warning' },
  'Ready':         { label: 'Prête',        color: 'info' },
  'InTransit':     { label: 'En livraison', color: 'primary' },
  'Delivered':     { label: 'Livrée',       color: 'success' },
  'Cancelled':     { label: 'Annulée',      color: 'danger' },

  // Services
  'Reserved':      { label: 'Réservée',     color: 'info' },
  'EnRoute':       { label: 'En route',     color: 'primary' },
  'OnSite':        { label: 'Sur place',    color: 'warning' },
  'InProgress':    { label: 'En cours',     color: 'warning' },
  'Completed':     { label: 'Terminée',     color: 'success' },
  'Disputed':      { label: 'Litige',       color: 'danger' },

  // Grocery
  'ProductUnavailable': { label: 'Produit indispo', color: 'warning' },

  // Payment
  'Pending':       { label: 'En attente',   color: 'warning' },
  'Paid':          { label: 'Payé',         color: 'success' },
  'Refunded':      { label: 'Remboursé',    color: 'info' },
  'Failed':        { label: 'Échoué',       color: 'danger' },

  // Verification
  'Approved':      { label: 'Approuvé',     color: 'success' },
  'Rejected':      { label: 'Rejeté',       color: 'danger' },
};

@Pipe({ name: 'statusBadge', standalone: true })
export class StatusBadgePipe implements PipeTransform {
  transform(status: string): StatusBadgeResult {
    return STATUS_MAP[status] ?? { label: status, color: 'secondary' };
  }
}