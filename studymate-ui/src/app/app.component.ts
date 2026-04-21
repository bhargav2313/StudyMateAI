import { Component } from '@angular/core';
import { PdfChatComponent } from './pdf-chat/pdf-chat.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [PdfChatComponent],
  template: `<app-pdf-chat></app-pdf-chat>`
})
export class AppComponent {}