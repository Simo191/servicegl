import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'statusBadge', standalone: true })
export class StatusBadgePipe implements PipeTransform {
  private colors: Record<string, string> = {
    'Pending': 'warning', 'Confirmed': 'info', 'Preparing': 'primary',
    'Ready': 'secondary', 'InTransit': 'info', 'Delivered': 'success',
    'Completed': 'success', 'Cancelled': 'danger', 'Refunded': 'dark',
    'Active': 'success', 'Inactive': 'secondary', 'Verified': 'success',
    'Unverified': 'warning', 'Online': 'success', 'Offline': 'secondary',
    'Paid': 'success', 'Unpaid': 'danger', 'Processing': 'info',
    'Reserved': 'warning', 'EnRoute': 'info', 'OnSite': 'primary',
    'InProgress': 'primary'
  };

  private labels: Record<string, string> = {
    'Pending': 'En attente', 'Confirmed': 'Confirmée', 'Preparing': 'En préparation',
    'Ready': 'Prête', 'InTransit': 'En route', 'Delivered': 'Livrée',
    'Completed': 'Terminée', 'Cancelled': 'Annulée', 'Refunded': 'Remboursée',
    'Active': 'Actif', 'Inactive': 'Inactif', 'Verified': 'Vérifié',
    'Unverified': 'Non vérifié', 'Online': 'En ligne', 'Offline': 'Hors ligne',
    'Paid': 'Payé', 'Unpaid': 'Impayé', 'Processing': 'En cours',
    'Reserved': 'Réservée', 'EnRoute': 'En route', 'OnSite': 'Sur place',
    'InProgress': 'En cours'
  };

  transform(value: string): { label: string; color: string } {
    return {
      label: this.labels[value] || value,
      color: this.colors[value] || 'secondary'
    };
  }
}
