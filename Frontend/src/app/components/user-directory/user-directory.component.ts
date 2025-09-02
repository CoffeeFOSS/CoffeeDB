import { Component, inject, OnInit } from '@angular/core';
import { UsersService } from '../../services/users.service';
import { User } from '../../models/user';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-directory',
  standalone: true, 
  imports: [CommonModule, RouterModule],
  templateUrl: './user-directory.component.html',
  styleUrl: './user-directory.component.scss'
})
export class UserDirectoryComponent implements OnInit {
  private usersService = inject(UsersService); 
  users?: User[];

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers() {
    this.usersService.getUsers().subscribe({
      next: (users) => {
        this.users = users;
      }
    })
  }
}
