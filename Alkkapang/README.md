<div align="center">
  <h1>🎮 알까팡_Scripts</h1>
  <p><b>뒤끝 서버 시스템 및 MVP 기반 UI 아키텍처 구현 저장소</b></p>
</div>

<br/>

<h2>📝 개요</h2>
<p>
  알까팡에서 저가 작성한 코드를 일부 옮겨 놓은 Repository입니다.
</p>

<hr/>

<h2>🚀 핵심 시스템 (Key Features)</h2>

<ul>
  <li>
    <b>서버 연동 및 유저 데이터 관리</b> (<code>The Backend</code> 활용)
    <ul>
      <li>로그인 전반 로직 처리 및 유저 세션 유지 관리</li>
      <li>뒤끝 매칭 서버를 활용한 대전 매칭</li>
    </ul>
  </li>
  <br/>
  <li>
    <b>MVP 패턴 기반 UI 프레임워크 설계</b>
    <ul>
      <li>Model-View-Presenter 구조를 통해 UI 데이터와 렌더링 로직의 의존성 완전 분리</li>
      <li>Scroll Snap UI를 MVP 구조로 설계하여 부드러운 인터랙션과 데이터 동기화 구현</li>
    </ul>
  </li>
</ul>

<hr/>

<h2>📁 주요 스크립트 구성</h2>

<table width="100%">
  <thead>
    <tr>
      <th align="left">Category</th>
      <th align="left">Scripts</th>
      <th align="left">Description</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><b>Backend / Auth</b></td>
      <td><code>BackendLogin.cs</code></td>
      <td>뒤끝 API를 이용한 유저 정보 관리 및 보안 로그인</td>
    </tr>
    <tr>
      <td><b>Match / Network</b></td>
      <td><code>BackEndMatchManager.cs</code><br/><code>NetworkManager.cs</code></td>
      <td>서버 대기열 매칭 프로세스</td>
    </tr>
    <tr>
      <td><b>UI System (MVP)</b></td>
      <td><code>MainModel.cs</code><br/><code>MainView.cs</code><br/><code>MainPresenter.cs</code></td>
      <td>MVP 패턴 기반의 Scroll Snap 메인 UI 컨트롤러</td>
    </tr>
  </tbody>
</table>

<hr/>
