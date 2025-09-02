import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-mainframe',
  standalone: true,
  imports: [RouterOutlet, RouterLink],
  templateUrl: './mainframe.component.html',
  styleUrl: './mainframe.component.scss',
})
export class MainframeComponent {}
