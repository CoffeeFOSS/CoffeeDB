import {
  Component,
  inject,
  signal,
  effect,
  untracked,
  OnInit,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UsersService } from '../../services/users.service';
import { LoadingService } from '../../services/loading.service';
import { Member } from '../../models/member';
import { PaginatedResult } from '../../models/pagination';
import { PaginationControlsComponent } from '../pagination-controls/pagination-controls.component';
import {
  buildParamsFromForm,
  buildParamsValueDefaults,
  createPaginationSignals,
  fetchItemsWithCache,
  sanitizeObjectFields,
  subscribeToQueryParams,
  syncParamsWithUrl,
} from '../../utils/params.utils';
import { getPaginationText } from '../../utils/pagination.utils';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { setSubmittedAndValidateForm } from '../../utils/form.utils';
import { TextInputComponent } from '../forms/text-input/text-input.component';

@Component({
  selector: 'app-user-directory',
  templateUrl: './user-directory.component.html',
  imports: [
    PaginationControlsComponent,
    ReactiveFormsModule,
    TextInputComponent,
  ],
})
export class UserDirectoryComponent implements OnInit {
  protected usersService = inject(UsersService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  protected cache: Record<string, PaginatedResult<Member[]>> = {};
  private fb = new FormBuilder();
  loadingService = inject(LoadingService);
  searchForm: FormGroup = new FormGroup({});
  validationErrors: string[] = [];
  paginationSignals = createPaginationSignals();
  users = signal<Member[]>([]);
  paginatedResultSignal = signal<PaginatedResult<Member[]> | null>(null);
  submitted = signal(false);
  loadingKey = 'user-directory';

  protected formKeyMap: Record<string, { paramCode: string; default: any }> = {
    username: { paramCode: 'u', default: '' },
  };

  constructor() {
    effect(() => {
      this.paginationSignals.page.signal();
      this.paginationSignals.pageSize.signal();
      untracked(() => {
        this.syncParamsWithUrl();
        this.fetchItems();
      });
    });
  }

  ngOnInit(): void {
    this.initializeForm();
    subscribeToQueryParams({
      route: this.route,
      formGroup: this.searchForm,
      paginationSignals: this.paginationSignals,
      formKeyMap: this.formKeyMap,
      submittedSignal: this.submitted,
      validationErrors: this.validationErrors,
    });
  }

  initializeForm() {
    const { username } = this.formKeyMap;
    this.searchForm = this.fb.group({
      username: [
        username.default,
        [Validators.maxLength(3), Validators.maxLength(20)],
      ],
    });
  }

  fetchItems() {
    fetchItemsWithCache({
      cache: this.cache,
      itemsSignal: this.users,
      paginationSignals: this.paginationSignals,
      params: buildParamsFromForm(this.searchForm.value, this.formKeyMap),
      loadingKey: this.loadingKey,
      loadingService: this.loadingService,
      resultSignal: this.paginatedResultSignal,
      fetchPaginatedItems: this.fetchPaginatedItems,
    });
  }

  fetchPaginatedItems = () =>
    this.usersService.getUsers({
      page: this.paginationSignals.page.signal(),
      pageSize: this.paginationSignals.pageSize.signal(),
      ...sanitizeObjectFields(this.searchForm.value),
    });

  syncParamsWithUrl() {
    syncParamsWithUrl({
      router: this.router,
      route: this.route,
      paginationSignals: this.paginationSignals,
      params: buildParamsValueDefaults(this.searchForm.value, this.formKeyMap),
    });
  }

  onSearchSubmit() {
    setSubmittedAndValidateForm(
      this.submitted,
      this.searchForm,
      this.validationErrors,
    );
    this.syncParamsWithUrl();
    this.fetchItems();
  }

  onResetSearch() {
    this.searchForm.reset();
    this.onSearchSubmit();
  }

  get paginationText(): string {
    return getPaginationText(this.paginatedResultSignal());
  }
}
