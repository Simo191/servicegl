import { Injectable, signal } from '@angular/core';

export interface Toast {
  id: number;
  type: 'success' | 'error' | 'warning' | 'info';
  message: string;
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  private _toasts = signal<Toast[]>([]);
  private counter = 0;
  readonly toasts = this._toasts.asReadonly();

  private show(type: Toast['type'], message: string, duration = 4000): void {
    const id = ++this.counter;
    this._toasts.update(t => [...t, { id, type, message }]);
    setTimeout(() => this.dismiss(id), duration);
  }

  success(msg: string): void { this.show('success', msg); }
  error(msg: string): void { this.show('error', msg, 6000); }
  warning(msg: string): void { this.show('warning', msg, 5000); }
  info(msg: string): void { this.show('info', msg); }

  dismiss(id: number): void {
    this._toasts.update(t => t.filter(x => x.id !== id));
  }
}
