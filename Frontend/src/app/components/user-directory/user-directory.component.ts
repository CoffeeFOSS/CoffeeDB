import { Component, inject, OnInit } from '@angular/core';
import { UsersService } from '../../services/users.service';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { PaginationControlsComponent } from '../pagination-controls/pagination-controls.component';

@Component({
  selector: 'app-user-directory',
  standalone: true,
  imports: [CommonModule, RouterModule, PaginationControlsComponent],
  templateUrl: './user-directory.component.html',
  styleUrl: './user-directory.component.scss',
})
export class UserDirectoryComponent implements OnInit {
  usersService = inject(UsersService);
  page = 1;
  pageSize = 5;
  pageSizeInput = 5; // is used as a temporary pageSize

  ngOnInit(): void {
    if (!this.usersService.paginatedResult()) {
      this.loadUsers();
    }
  }

  loadUsers() {
    this.usersService.getUsers(this.page, this.pageSize);
  }

  onPageUpdate(newPage: number) {
    this.page = newPage;
  }

  onPageChange(newPage: number) {
    if (
      newPage < 1 ||
      newPage > this.usersService.paginatedResult()?.pagination?.totalPages!
    ) {
      return;
    }
    this.page = newPage;
    this.loadUsers();
  }

  onPageSizeInput($event: Event) {
    this.pageSizeInput = Number(($event.target as HTMLInputElement).value);
  }

  onPageSizeChange($event: number) {
    this.pageSize = $event;
    this.page = 1;
    // this.loadUsers();
  }
}
