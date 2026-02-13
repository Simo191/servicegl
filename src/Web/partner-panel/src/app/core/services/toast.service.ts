import { Injectable, signal } from '@angular/core';
export interface Toast { id: number; message: string; type: 'success'|'error'|'warning'|'info'; }
@Injectable({ providedIn: 'root' })
export class ToastService {
  private c = 0; toasts = signal<Toast[]>([]);
  show(msg: string, type: Toast['type'] = 'info', dur = 4000): void { const t = { id: ++this.c, message: msg, type }; this.toasts.update(a => [...a, t]); setTimeout(() => this.remove(t.id), dur); }
  success(m: string) { this.show(m,'success'); } error(m: string) { this.show(m,'error',6000); }
  warning(m: string) { this.show(m,'warning'); } info(m: string) { this.show(m,'info'); }
  remove(id: number) { this.toasts.update(a => a.filter(x => x.id !== id)); }
}
