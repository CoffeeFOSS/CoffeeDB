import {
  Component,
  effect,
  inject,
  signal,
  OnInit,
  OnDestroy,
} from '@angular/core';
import { UsersService } from '../../services/users.service';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { PaginationControlsComponent } from '../pagination-controls/pagination-controls.component';
import { Subscription } from 'rxjs';
import { Member } from '../../models/member';
import { PaginatedResult } from '../../models/pagination';

@Component({
  selector: 'app-user-directory',
  standalone: true,
  imports: [CommonModule, RouterModule, PaginationControlsComponent],
  templateUrl: './user-directory.component.html',
  styleUrl: './user-directory.component.scss',
})
export class UserDirectoryComponent implements OnInit, OnDestroy {
  usersService = inject(UsersService);
  router = inject(Router);
  route = inject(ActivatedRoute);
  page = signal(1);
  pageSize = signal(20);
  private skipInitialFetch = false;
  private lastFetchedPage = -1;
  private subscriptions = new Subscription();

  constructor() {
    console.log('CONSTRUCTOR');
    // --- 1. POPSTATE LISTENER: The most reliable cache check for BACK button ---
    // This listener fires when the browser's history entry changes (Back/Forward buttons).
    const popstateListener = (event: PopStateEvent) => {
      const cachedUsers: PaginatedResult<Member[]> | undefined =
        event.state?.cachedUsers;

      if (cachedUsers) {
        console.log('POPSTATE CACHE HIT! Loading data and skipping fetch.');
        this.usersService.paginatedResult.set(cachedUsers);
        this.skipInitialFetch = true;
      }
    };

    // Attach the listener and make sure to clean it up.
    window.addEventListener('popstate', popstateListener);
    this.subscriptions.add(
      new Subscription(() =>
        window.removeEventListener('popstate', popstateListener),
      ),
    );

    // 2. Parse URL and update page signal
    // This runs on init and when URL changes (including after popstate).
    this.route.queryParamMap.subscribe((params) => {
      const pageParam = Number(params.get('page'));
      if (!isNaN(pageParam) && pageParam > 0 && pageParam !== this.page()) {
        console.log(`URL changed, setting page signal to: ${pageParam}`);
        this.page.set(pageParam);
      }
    });

    // 3. Data Fetch Effect: Runs whenever 'page' signal changes
    effect(() => {
      if (this.skipInitialFetch) {
        console.log('Effect skipped due to cached state.');
        // Reset flag *after* the fetch is skipped to allow future page clicks to fetch.
        this.skipInitialFetch = false;
        return;
      }

      const currentPage = this.page();
      const currentSize = this.pageSize();
      console.log('API FETCH triggered.');
      this.usersService.getUsers(currentPage, currentSize);
      this.lastFetchedPage = currentPage;
    });

    // 4. Update History and Save Cache State ONLY when the result arrives
    // Only run this ONCE per paginatedResult() changes. If we add currenPage = this.page(),
    // This ends up running twice and breaking the history states.
    effect(() => {
      const currentResult = this.usersService.paginatedResult();

      // Only save if we have a result AND it corresponds to a forward fetch
      if (currentResult && !this.skipInitialFetch) {
        const pageFromResult = currentResult.pagination?.currentPage;

        const urlTree = this.router.createUrlTree([], {
          relativeTo: this.route,
          queryParams: { page: pageFromResult },
        });
        const url = this.router.serializeUrl(urlTree);

        // Use window.history.pushState to save the cache data.
        window.history.pushState(
          {
            ...window.history.state,
            cachedUsers: currentResult,
          },
          '',
          url,
        );
        console.log(
          `History pushState executed for page ${pageFromResult}. Cache saved.`,
        );
      }
    });
  }

  // Use ngOnInit to perform an initial cache check if the component was just created
  ngOnInit(): void {
    // This handles initial page load where state might be present from a router redirect
    const cachedUsers: PaginatedResult<Member[]> | undefined =
      window.history.state?.cachedUsers;

    if (cachedUsers) {
      console.log('NGONINIT CACHE HIT! Loading data and skipping fetch.');
      this.usersService.paginatedResult.set(cachedUsers);
      this.skipInitialFetch = true;
    }
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }
}
